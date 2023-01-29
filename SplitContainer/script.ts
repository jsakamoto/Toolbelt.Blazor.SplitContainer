const enum Direction {
    Horizontal,
    Virtical
}

const pointerdown = "pointerdown";
const pointermove = "pointermove";
const pointerup = "pointerup";
const touchstart = "touchstart";
const NULL = null;

interface DotNetObjectRef {
    invokeMethodAsync(methodName: string, ...args: any[]): Promise<any>
}

export const attach = (component: DotNetObjectRef, container: HTMLElement) => {
    const state = {
        dir: Direction.Horizontal,
        resizeTarget: -1,
        pivot: 0,
        initSize: 0,
        disposed: false,
        dispose: NULL as any
    };
    const spliter = container.querySelector(":scope > .spliter-bar")! as HTMLElement;
    const panes = Array.from(container.querySelectorAll(":scope > .pane-of-split-container")) as HTMLElement[];

    const round = Math.round;

    const getPos = (dir: Direction, ev: PointerEvent): number => round(dir === Direction.Horizontal ? ev.clientX : ev.clientY);

    const getSize = (dir: Direction, targetPaneIndex: number): number => round(((rect: DOMRect) => dir === Direction.Horizontal ? rect.width : rect.height)(panes[targetPaneIndex].getBoundingClientRect()));

    const addEventListener = (element: HTMLElement, event: string, callback: Function, options?: AddEventListenerOptions) => {
        element.addEventListener(event as any, callback as any, options);
    }

    const removeEventListener = (element: HTMLElement, event: string, callback: Function) => {
        element.removeEventListener(event as any, callback as any);
    }

    const updateSize = (ev: PointerEvent): [HTMLElement | null, number] => {
        const targetPaneIndex = state.resizeTarget;
        const resizeTarget = panes[targetPaneIndex] || NULL;
        if (resizeTarget === NULL) return [NULL, 0];

        const resizeTargetStyle = resizeTarget.style;
        const dir = state.dir;
        const currentPos = getPos(dir, ev);
        const delta = currentPos - state.pivot;
        const nextSize = (state.initSize + (targetPaneIndex == 0 ? +1 : -1) * delta) + "px";
        if (dir === Direction.Horizontal)
            resizeTargetStyle.width = nextSize;
        else
            resizeTargetStyle.height = nextSize;

        return [resizeTarget, getSize(dir, targetPaneIndex)];
    }

    const onPointerMove = (ev: PointerEvent): void => { updateSize(ev); }

    const onPointerDown = (ev: PointerEvent): void => {
        if (!document.contains(spliter)) { state.dispose(); return; }
        const targetPaneIndex = panes.findIndex(p => p.style.flex === "");
        if (targetPaneIndex === -1) return;
        const dir = container.classList.contains("splitter-orientation-vertical") ? Direction.Horizontal : Direction.Virtical;

        state.dir = dir;
        state.resizeTarget = targetPaneIndex;
        state.pivot = getPos(dir, ev);
        state.initSize = getSize(dir, targetPaneIndex);
        addEventListener(spliter, pointermove, onPointerMove);
        spliter.setPointerCapture(ev.pointerId);
    }

    const onPointerUp = (ev: PointerEvent): void => {
        spliter.releasePointerCapture(ev.pointerId);
        removeEventListener(spliter, pointermove, onPointerMove);
        const [resizeTarget, nextSize] = updateSize(ev);
        component.invokeMethodAsync("UpdateSize", resizeTarget === panes[0], nextSize);
    }

    const preventDefault = (ev: TouchEvent): void => ev.preventDefault();

    addEventListener(spliter, pointerdown, onPointerDown);
    addEventListener(spliter, pointerup, onPointerUp);
    addEventListener(spliter, touchstart, preventDefault, { passive: false });

    state.dispose = () => {
        if (state.disposed) return;
        state.disposed = true;
        removeEventListener(spliter, pointerdown, onPointerDown);
        removeEventListener(spliter, pointerup, onPointerUp);
        removeEventListener(spliter, touchstart, preventDefault);
    };
    return state;
}
