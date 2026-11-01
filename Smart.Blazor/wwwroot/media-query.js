export function observe(dotNetRef, query) {
    let list = window.matchMedia(query);
    const handler = (e) => dotNetRef.invokeMethodAsync("NotifyChange", e.matches);
    list.addEventListener("change", handler);

    return {
        matches: () => list.matches,
        update: (value) => {
            list.removeEventListener("change", handler);
            list = window.matchMedia(value);
            list.addEventListener("change", handler);
            return list.matches;
        },
        dispose: () => list.removeEventListener("change", handler)
    };
}
