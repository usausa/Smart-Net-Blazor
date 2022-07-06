"use strict";
var Smart;
(function (smart) {
    smart.setFocus = (id) => {
        const control = document.getElementById(id);
        if (control) {
            control.focus();
        }
    }

    smart.saveAsFile = (filename, contentType, base64) => {
        const link = document.createElement("a");
        link.href = `data:${contentType};base64,${base64}`;
        link.target = "_self";
        link.download = filename;
        link.click();
        link.remove();
    }

    smart.openNewWindow = (contentType, base64) => {
        const sliceSize = 65536;

        const data = atob(base64);
        const bytes = [];
        for (let offset = 0; offset < data.length; offset += sliceSize) {
            const slice = data.slice(offset, offset + sliceSize);
            const array = new Array(slice.length);
            for (let i = 0; i < slice.length; i++) {
                array[i] = slice.charCodeAt(i);
            }

            bytes.push(new Uint8Array(array));
        }

        const blob = new Blob(bytes, { type: contentType });
        const url = URL.createObjectURL(blob);
        window.open(url);
    }
})(Smart || (Smart = {}));
