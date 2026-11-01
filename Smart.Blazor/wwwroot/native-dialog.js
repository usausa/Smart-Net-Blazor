export function attach(dialog, dotNetRef, closeOnEscape, closeOnBackdropClick) {
    let options = { closeOnEscape, closeOnBackdropClick };
    const onClose = () => dotNetRef.invokeMethodAsync("NotifyClose", dialog.returnValue);
    const onCancel = (e) => {
        if (!options.closeOnEscape) {
            e.preventDefault();
        }
    };
    const onClick = (e) => {
        if (!options.closeOnBackdropClick || (e.target !== dialog) || !dialog.matches(":modal")) {
            return;
        }
        const rect = dialog.getBoundingClientRect();
        if ((e.clientX < rect.left) || (e.clientX > rect.right) || (e.clientY < rect.top) || (e.clientY > rect.bottom)) {
            dialog.close();
        }
    };
    dialog.addEventListener("close", onClose);
    dialog.addEventListener("cancel", onCancel);
    dialog.addEventListener("click", onClick);

    return {
        show: (modal) => {
            if (dialog.open) {
                return;
            }
            dialog.returnValue = "";
            if (modal) {
                dialog.showModal();
            } else {
                dialog.show();
            }
        },
        close: () => {
            if (dialog.open) {
                dialog.close();
            }
        },
        update: (escape, backdropClick) => {
            options = { closeOnEscape: escape, closeOnBackdropClick: backdropClick };
        },
        dispose: () => {
            dialog.removeEventListener("close", onClose);
            dialog.removeEventListener("cancel", onCancel);
            dialog.removeEventListener("click", onClick);
        }
    };
}
