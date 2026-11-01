const isEditable = (e) => {
    const target = e.composedPath()[0];
    return (target instanceof HTMLElement) && (target.isContentEditable || target.matches("input, textarea, select"));
};

const createOptions = (key, ctrl, alt, shift, meta, preventDefault, allowInInput, allowRepeat) =>
    ({ key, ctrl, alt, shift, meta, preventDefault, allowInInput, allowRepeat });

export function register(dotNetRef, ...args) {
    let options = createOptions(...args);
    const handler = (e) => {
        if (e.isComposing || (e.keyCode === 229)) {
            return;
        }
        if (e.repeat && !options.allowRepeat) {
            return;
        }
        if ((typeof e.key !== "string") || (e.key.toUpperCase() !== options.key)) {
            return;
        }
        if ((e.ctrlKey !== options.ctrl) || (e.altKey !== options.alt) || (e.shiftKey !== options.shift) || (e.metaKey !== options.meta)) {
            return;
        }
        if (!options.allowInInput && isEditable(e)) {
            return;
        }
        if (options.preventDefault) {
            e.preventDefault();
        }
        dotNetRef.invokeMethodAsync("NotifyPressed");
    };
    document.addEventListener("keydown", handler);

    return {
        update: (...values) => {
            options = createOptions(...values);
        },
        dispose: () => document.removeEventListener("keydown", handler)
    };
}
