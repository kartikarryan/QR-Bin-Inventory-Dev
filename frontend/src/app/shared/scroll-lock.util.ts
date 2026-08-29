/** Reference-counted so nested/simultaneous overlays (e.g. a return dialog opened on
 *  top of a bill view modal) don't unlock the body when only the inner one closes. */
let lockCount = 0;

export function lockBodyScroll(): () => void {
  lockCount++;
  document.body.style.overflow = 'hidden';

  let released = false;
  return () => {
    if (released) return;
    released = true;

    lockCount = Math.max(0, lockCount - 1);
    if (lockCount === 0) {
      document.body.style.overflow = '';
    }
  };
}
