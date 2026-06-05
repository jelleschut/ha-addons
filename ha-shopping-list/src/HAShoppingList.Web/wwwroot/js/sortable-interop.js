window.initSortable = (elementId, dotNetRef, methodName = 'OnOrderChanged', extraArg = null) => {
    const el = document.getElementById(elementId);
    if (!el) return;
    if (el._sortable) el._sortable.destroy();

    el._sortable = Sortable.create(el, {
        animation: 150,
        handle: '.drag-handle',
        onEnd: (evt) => {
            const { oldIndex, newIndex } = evt;
            if (oldIndex === newIndex) return;

            if (extraArg !== null)
                dotNetRef.invokeMethodAsync(methodName, oldIndex, newIndex, extraArg);
            else
                dotNetRef.invokeMethodAsync(methodName, oldIndex, newIndex);
        }
    });
};

window.destroySortable = (elementId) => {
    const el = document.getElementById(elementId);
    if (el?._sortable) {
        el._sortable.destroy();
        el._sortable = null;
    }
};
