// Generates public/icons/icon-{size}.png for the PWA manifest, matching the favicon design
// (blue rounded square + white QR-corner glyph). Anti-aliased via 4x supersampling. No deps.
const fs = require('fs');
const path = require('path');
const zlib = require('zlib');

const SIZES = [72, 96, 128, 144, 152, 192, 384, 512];
const SS = 4; // supersample factor for anti-aliasing
const BG = [0x1d, 0x4e, 0xd8, 255];
const WHITE = [0xff, 0xff, 0xff, 255];
const TRANSPARENT = [0, 0, 0, 0];

// All geometry below is expressed in the same 32-unit design space as favicon.svg.
function inRoundedRect(x, y, ox, oy, w, h, r) {
  const lx = x - ox;
  const ly = y - oy;
  if (lx < 0 || ly < 0 || lx >= w || ly >= h) return false;
  const cx = lx < r ? r : lx >= w - r ? w - r : lx;
  const cy = ly < r ? r : ly >= h - r ? h - r : ly;
  if (lx >= r && lx < w - r) return true;
  if (ly >= r && ly < h - r) return true;
  const dx = lx - cx;
  const dy = ly - cy;
  return dx * dx + dy * dy <= r * r;
}

function strokeSquare(x, y, ox, oy, size, stroke) {
  const inOuter = inRoundedRect(x, y, ox, oy, size, size, 1.5);
  const inInner = x >= ox + stroke && x < ox + size - stroke && y >= oy + stroke && y < oy + size - stroke;
  return inOuter && !inInner;
}

function filledSquare(x, y, ox, oy, size) {
  return x >= ox && x < ox + size && y >= oy && y < oy + size;
}

function colorAt(x, y) {
  // x, y in 0..32 design units
  if (!inRoundedRect(x, y, 0, 0, 32, 32, 7)) return TRANSPARENT;

  const lx = x - 4;
  const ly = y - 4;
  if (strokeSquare(lx, ly, 2, 2, 7, 2)) return WHITE;
  if (strokeSquare(lx, ly, 15, 2, 7, 2)) return WHITE;
  if (strokeSquare(lx, ly, 2, 15, 7, 2)) return WHITE;
  if (filledSquare(lx, ly, 15, 15, 3)) return WHITE;
  if (filledSquare(lx, ly, 19, 15, 3)) return WHITE;
  if (filledSquare(lx, ly, 15, 19, 3)) return WHITE;
  return BG;
}

function renderIcon(size) {
  const buf = Buffer.alloc(size * size * 4);
  for (let py = 0; py < size; py++) {
    for (let px = 0; px < size; px++) {
      let r = 0, g = 0, b = 0, a = 0;
      for (let sy = 0; sy < SS; sy++) {
        for (let sx = 0; sx < SS; sx++) {
          const x = ((px + (sx + 0.5) / SS) / size) * 32;
          const y = ((py + (sy + 0.5) / SS) / size) * 32;
          const [cr, cg, cb, ca] = colorAt(x, y);
          r += cr * ca;
          g += cg * ca;
          b += cb * ca;
          a += ca;
        }
      }
      const n = SS * SS;
      const outA = a / n;
      const offset = (py * size + px) * 4;
      if (outA > 0) {
        buf[offset] = Math.round(r / a);
        buf[offset + 1] = Math.round(g / a);
        buf[offset + 2] = Math.round(b / a);
      }
      buf[offset + 3] = Math.round(outA);
    }
  }
  return buf;
}

function crc32(buf) {
  let c;
  const table = crc32.table || (crc32.table = (() => {
    const t = new Uint32Array(256);
    for (let n = 0; n < 256; n++) {
      c = n;
      for (let k = 0; k < 8; k++) c = c & 1 ? 0xedb88320 ^ (c >>> 1) : c >>> 1;
      t[n] = c >>> 0;
    }
    return t;
  })());
  let crc = 0xffffffff;
  for (let i = 0; i < buf.length; i++) crc = table[(crc ^ buf[i]) & 0xff] ^ (crc >>> 8);
  return (crc ^ 0xffffffff) >>> 0;
}

function chunk(type, data) {
  const typeBuf = Buffer.from(type, 'ascii');
  const lenBuf = Buffer.alloc(4);
  lenBuf.writeUInt32BE(data.length, 0);
  const crcBuf = Buffer.alloc(4);
  crcBuf.writeUInt32BE(crc32(Buffer.concat([typeBuf, data])), 0);
  return Buffer.concat([lenBuf, typeBuf, data, crcBuf]);
}

function encodePng(rgba, size) {
  const sig = Buffer.from([0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a]);

  const ihdr = Buffer.alloc(13);
  ihdr.writeUInt32BE(size, 0);
  ihdr.writeUInt32BE(size, 4);
  ihdr[8] = 8; // bit depth
  ihdr[9] = 6; // color type: RGBA
  ihdr[10] = 0;
  ihdr[11] = 0;
  ihdr[12] = 0;

  const raw = Buffer.alloc(size * (size * 4 + 1));
  for (let y = 0; y < size; y++) {
    raw[y * (size * 4 + 1)] = 0; // filter type: None
    rgba.copy(raw, y * (size * 4 + 1) + 1, y * size * 4, (y + 1) * size * 4);
  }
  const idat = zlib.deflateSync(raw);

  return Buffer.concat([sig, chunk('IHDR', ihdr), chunk('IDAT', idat), chunk('IEND', Buffer.alloc(0))]);
}

const outDir = path.join(__dirname, '..', 'public', 'icons');
for (const size of SIZES) {
  const rgba = renderIcon(size);
  const png = encodePng(rgba, size);
  const dest = path.join(outDir, `icon-${size}x${size}.png`);
  fs.writeFileSync(dest, png);
  console.log('Wrote', dest, png.length, 'bytes');
}
