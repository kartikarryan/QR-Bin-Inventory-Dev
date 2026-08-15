// One-off generator for public/favicon.ico, matching public/favicon.svg
// (blue rounded square + white QR-corner glyph). No external deps.
const fs = require('fs');
const path = require('path');

const SIZE = 32;
const BG = [0x1d, 0x4e, 0xd8]; // #1d4ed8
const WHITE = [0xff, 0xff, 0xff];
const OFFSET = 4; // <g transform="translate(4,4)"> in the SVG

function inRoundedRect(x, y, w, h, r) {
  if (x < 0 || y < 0 || x >= w || y >= h) return false;
  const cx = x < r ? r : x >= w - r ? w - r - 1 : x;
  const cy = y < r ? r : y >= h - r ? h - r - 1 : y;
  if (x >= r && x < w - r) return true;
  if (y >= r && y < h - r) return true;
  const dx = x - cx;
  const dy = y - cy;
  return dx * dx + dy * dy <= r * r;
}

// Stroke-only rounded square: inside the outer rounded rect but outside the (rounded-ish) inner cutout.
function strokeSquare(lx, ly, ox, oy, size, stroke) {
  const inOuter = inRoundedRect(lx - ox, ly - oy, size, size, 1.5);
  const inInner = lx >= ox + stroke && lx < ox + size - stroke && ly >= oy + stroke && ly < oy + size - stroke;
  return inOuter && !inInner;
}

function filledSquare(lx, ly, ox, oy, size) {
  return lx >= ox && lx < ox + size && ly >= oy && ly < oy + size;
}

const pixels = [];
for (let y = 0; y < SIZE; y++) {
  for (let x = 0; x < SIZE; x++) {
    let color = null;

    if (inRoundedRect(x, y, SIZE, SIZE, 7)) {
      color = BG;

      const lx = x - OFFSET;
      const ly = y - OFFSET;

      if (strokeSquare(lx, ly, 2, 2, 7, 2)) color = WHITE;
      if (strokeSquare(lx, ly, 15, 2, 7, 2)) color = WHITE;
      if (strokeSquare(lx, ly, 2, 15, 7, 2)) color = WHITE;
      if (filledSquare(lx, ly, 15, 15, 3)) color = WHITE;
      if (filledSquare(lx, ly, 19, 15, 3)) color = WHITE;
      if (filledSquare(lx, ly, 15, 19, 3)) color = WHITE;
    }

    pixels.push(color);
  }
}

// Build a 32bpp BMP (BITMAPINFOHEADER) for the ICO image data: bottom-up row order, BGRA.
const rowSize = SIZE * 4;
const imageDataSize = rowSize * SIZE;
const andMaskRowSize = Math.ceil(SIZE / 32) * 4;
const andMaskSize = andMaskRowSize * SIZE;

const bmpHeaderSize = 40;
const bmp = Buffer.alloc(bmpHeaderSize + imageDataSize + andMaskSize);
let o = 0;
bmp.writeUInt32LE(bmpHeaderSize, o); o += 4; // biSize
bmp.writeInt32LE(SIZE, o); o += 4; // biWidth
bmp.writeInt32LE(SIZE * 2, o); o += 4; // biHeight (doubled: XOR + AND masks)
bmp.writeUInt16LE(1, o); o += 2; // biPlanes
bmp.writeUInt16LE(32, o); o += 2; // biBitCount
bmp.writeUInt32LE(0, o); o += 4; // biCompression = BI_RGB
bmp.writeUInt32LE(imageDataSize, o); o += 4; // biSizeImage
bmp.writeInt32LE(0, o); o += 4; // biXPelsPerMeter
bmp.writeInt32LE(0, o); o += 4; // biYPelsPerMeter
bmp.writeUInt32LE(0, o); o += 4; // biClrUsed
bmp.writeUInt32LE(0, o); o += 4; // biClrImportant

for (let y = SIZE - 1; y >= 0; y--) {
  for (let x = 0; x < SIZE; x++) {
    const c = pixels[y * SIZE + x];
    const [r, g, b, a] = c ? [c[0], c[1], c[2], 255] : [0, 0, 0, 0];
    bmp.writeUInt8(b, o); o += 1;
    bmp.writeUInt8(g, o); o += 1;
    bmp.writeUInt8(r, o); o += 1;
    bmp.writeUInt8(a, o); o += 1;
  }
}
// AND mask: all zero (fully opaque where alpha channel already handles transparency).
o += andMaskSize;

const icoHeader = Buffer.alloc(6);
icoHeader.writeUInt16LE(0, 0);
icoHeader.writeUInt16LE(1, 2);
icoHeader.writeUInt16LE(1, 4);

const dirEntry = Buffer.alloc(16);
dirEntry.writeUInt8(SIZE, 0);
dirEntry.writeUInt8(SIZE, 1);
dirEntry.writeUInt8(0, 2);
dirEntry.writeUInt8(0, 3);
dirEntry.writeUInt16LE(1, 4);
dirEntry.writeUInt16LE(32, 6);
dirEntry.writeUInt32LE(bmp.length, 8);
dirEntry.writeUInt32LE(icoHeader.length + dirEntry.length, 12);

const out = Buffer.concat([icoHeader, dirEntry, bmp]);
const dest = path.join(__dirname, '..', 'public', 'favicon.ico');
fs.writeFileSync(dest, out);
console.log('Wrote', dest, out.length, 'bytes');
