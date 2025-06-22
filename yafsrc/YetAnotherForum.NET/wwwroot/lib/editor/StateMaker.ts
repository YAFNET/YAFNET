/* StateMaker created by Jason Raymond Buckley */

export default class StateMaker {
    initialState?: string;
    states: string[];
    savedStates: string[];
    canUndo: boolean;
    canRedo: boolean;
    undoneStates: string[];

    constructor(initialState?: string) {
        this.initialState = initialState;
        this.states = initialState ? [initialState] : [];
        this.savedStates = [];
        this.canUndo = false;
        this.canRedo = false;
        this.undoneStates = [];
    }

    addState(state: string): this {
        this.states.push(state);
        this.undoneStates = [];
        this.canUndo = true;
        this.canRedo = false;
        return this;
    }

    undo(): this {
        const sl = this.states.length;
        if (this.initialState) {
            if (sl > 1) {
                this.undoneStates.push(this.states.pop()!);
                this.canRedo = true;
                this.canUndo = this.states.length >= 2;
            } else {
                this.canUndo = false;
            }
        } else if (sl > 0) {
            this.undoneStates.push(this.states.pop()!);
            this.canRedo = true;
            this.canUndo = this.states.length > 0;
        } else {
            this.canUndo = false;
        }
        return this;
    }

    redo(): this {
        if (this.undoneStates.length > 0) {
            this.states.push(this.undoneStates.pop()!);
            this.canUndo = true;
            this.canRedo = this.undoneStates.length > 0;
        } else {
            this.canRedo = false;
        }
        return this;
    }

    // test to see if current state in use is a saved state
    save(): this {
        this.savedStates = [...this.states];
        return this;
    }

    isSavedState(): boolean {
        return JSON.stringify(this.states) === JSON.stringify(this.savedStates);
    }
}