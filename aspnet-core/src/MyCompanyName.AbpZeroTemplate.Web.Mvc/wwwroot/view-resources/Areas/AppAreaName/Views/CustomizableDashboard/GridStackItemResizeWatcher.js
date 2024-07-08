class GridStackItemResizeWatcher {
    constructor(targetNode, onResizeStartedCallback, onResizeEndedCallback) {
        this.classToWatch = 'ui-resizable-resizing';
        this.targetNode = targetNode;
        this.onResizeStartedCallback = onResizeStartedCallback;
        this.onResizeEndedCallback = onResizeEndedCallback;
        this.observer = null;
        this.lastClassState = targetNode.classList.contains(this.classToWatch);

        this.init();
    }

    init() {
        this.observer = new MutationObserver(this.mutationCallback);
        this.observe();
    }

    observe() {
        this.observer.observe(this.targetNode, { attributes: true });
    }

    disconnect() {
        this.observer.disconnect();
    }

    mutationCallback = mutationsList => {
        for(let mutation of mutationsList) {
            if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                let currentClassState = mutation.target.classList.contains(this.classToWatch)
                if(this.lastClassState !== currentClassState) {
                    this.lastClassState = currentClassState
                    if(currentClassState) {
                        if (typeof this.onResizeStartedCallback === 'function') {
                            this.onResizeStartedCallback();
                        }
                    }
                    else {
                        if (typeof this.onResizeEndedCallback === 'function') {
                            this.onResizeEndedCallback()
                        }
                    }
                }
            }
        }
    }
}