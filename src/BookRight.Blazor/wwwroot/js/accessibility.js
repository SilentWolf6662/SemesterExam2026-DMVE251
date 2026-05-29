window.accessibility = {
    _trapHandlers: new WeakMap(),

    trapFocus(element) {
        const focusable = element.querySelectorAll(
            'a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])'
        );
        if (!focusable.length) return;

        const first = focusable[0];
        const last = focusable[focusable.length - 1];

        const handler = (e) => {
            if (e.key !== 'Tab') return;
            if (e.shiftKey) {
                if (document.activeElement === first) {
                    e.preventDefault();
                    last.focus();
                }
            } else {
                if (document.activeElement === last) {
                    e.preventDefault();
                    first.focus();
                }
            }
        };

        element.addEventListener('keydown', handler);
        this._trapHandlers.set(element, handler);
    },

    releaseFocus(element) {
        const handler = this._trapHandlers.get(element);
        if (handler) {
            element.removeEventListener('keydown', handler);
            this._trapHandlers.delete(element);
        }
    }
};
