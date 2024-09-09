const enum Direction {
    Horizontal,
    Virtical
}

const enum UnitOfSize {
    Pixel,
    Percent
}

const enum Props {
    Dir,
    TargetPaneIndex,
    PivotPos,
    InitSize,
    Unit,
    Disposed,
}

type TargetPaneIndex = 0 | 1;
type Position = number & { readonly brand: unique symbol };
type Size = number & { readonly brand: unique symbol };

type State = {
    [Props.Dir]: Direction;
    [Props.TargetPaneIndex]: TargetPaneIndex;
    [Props.PivotPos]: Position;
    [Props.InitSize]: Size;
    [Props.Unit]: UnitOfSize;
    [Props.Disposed]: boolean;
};

const pointerdown = "pointerdown";
const pointermove = "pointermove";
const pointerup = "pointerup";
const touchstart = "touchstart";
const NULL = null;

interface DotNetObjectRef {
    invokeMethodAsync(methodName: string, ...args: any[]): Promise<any>
}

export const attach = (component: DotNetObjectRef, container: HTMLElement) => {

    const state: State = [Direction.Horizontal, 0, 0 as Position, 0 as Size, UnitOfSize.Pixel, false];

    const splitter = container.querySelector(":scope > .spliter-bar")! as HTMLElement;
    const panes = [...container.querySelectorAll(":scope > .pane-of-split-container")] as [HTMLElement, HTMLElement];

    const round = Math.round;
    const getPos = (ev: PointerEvent): Position => round(state[Props.Dir] === Direction.Horizontal ? ev.clientX : ev.clientY) as Position;
    const getSize = (element: HTMLElement): Size => round(((rect: DOMRect) => state[Props.Dir] === Direction.Horizontal ? rect.width : rect.height)(element.getBoundingClientRect())) as Size;
    const getSizeOfPane = (targetPaneIndex: number): Size => getSize(panes[targetPaneIndex]);
    const addEventListenerToSplitter = splitter.addEventListener.bind(splitter);
    const removeEventListenerToSplitter = splitter.removeEventListener.bind(splitter);
    const preventDefault = (ev: TouchEvent): void => ev.preventDefault();

    const dispose = () => {
        if (state[Props.Disposed]) return;
        state[Props.Disposed] = true;
        removeEventListenerToSplitter(pointerdown, onPointerDown);
        removeEventListenerToSplitter(pointerup, onPointerUp);
        removeEventListenerToSplitter(touchstart, preventDefault);
    };

    const updateSize = (ev: PointerEvent): [HTMLElement | null, number] => {
        const targetPaneIndex = state[Props.TargetPaneIndex];
        const resizeTarget = panes[targetPaneIndex] || NULL;
        if (resizeTarget === NULL) return [NULL, 0];

        const resizeTargetStyle = resizeTarget.style;
        const currentPos = getPos(ev);
        const delta = currentPos - state[Props.PivotPos];
        const nextPxSize = (state[Props.InitSize] + (targetPaneIndex == 0 ? +1 : -1) * delta);
        const nextUnitSize = state[Props.Unit] === UnitOfSize.Percent ?
            parseFloat(((nextPxSize + getSize(splitter) / 2) / getSize(container) * 100).toFixed(3)) :
            nextPxSize;
        const nextSize = state[Props.Unit] === UnitOfSize.Percent ?
            `calc(${nextUnitSize}% - calc(var(--splitter-bar-size) / 2))` :
            nextUnitSize + "px";

        if (state[Props.Dir] === Direction.Horizontal)
            resizeTargetStyle.width = nextSize;
        else
            resizeTargetStyle.height = nextSize;

        return [resizeTarget, nextUnitSize];
    }

    const onPointerMove = (ev: PointerEvent): void => { updateSize(ev); }

    const onPointerDown = (ev: PointerEvent): void => {
        if (!document.contains(splitter)) { dispose(); return; }
        const targetPaneIndex = panes.findIndex(p => p.style.flex === "") as -1 | 0 | 1;
        if (targetPaneIndex === -1) return;
        state[Props.Dir] = container.classList.contains("splitter-orientation-vertical") ? Direction.Horizontal : Direction.Virtical;
        state[Props.TargetPaneIndex] = targetPaneIndex;
        state[Props.PivotPos] = getPos(ev);
        state[Props.InitSize] = getSizeOfPane(targetPaneIndex);
        state[Props.Unit] = container.dataset.unitOfSize === "percent" ? UnitOfSize.Percent : UnitOfSize.Pixel;
        addEventListenerToSplitter(pointermove, onPointerMove);
        splitter.setPointerCapture(ev.pointerId);
    }

    const onPointerUp = (ev: PointerEvent): void => {
        splitter.releasePointerCapture(ev.pointerId);
        removeEventListenerToSplitter(pointermove, onPointerMove);
        const [resizeTarget, nextUnitSize] = updateSize(ev);
        component.invokeMethodAsync("UpdateSize", resizeTarget === panes[0], nextUnitSize);
    }

    addEventListenerToSplitter(pointerdown, onPointerDown);
    addEventListenerToSplitter(pointerup, onPointerUp);
    addEventListenerToSplitter(touchstart, preventDefault, { passive: false });

    return { dispose: dispose };
}
