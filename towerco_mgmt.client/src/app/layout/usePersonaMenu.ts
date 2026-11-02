import { useCallback, useEffect, useState } from 'react';
import type { KeyboardEvent } from 'react';

export const PERSONA_ELEMENT_IDS = {
    root: 'persona-switcher',
    label: 'persona-switcher-label',
    trigger: 'persona-switcher-trigger',
    list: 'persona-switcher-list',
} as const;

interface MenuOptions {
    count: number;
    selectedIndex: number;
    onSelect: (index: number) => void;
}

interface KeyContext {
    count: number;
    activeIndex: number;
    setActiveIndex: (index: number) => void;
    choose: (index: number) => void;
    close: () => void;
}

const KEY_STEPS: Record<string, number> = { ArrowDown: 1, ArrowUp: -1 };
const CHOOSE_KEYS = ['Enter', ' '];
const CLOSE_KEYS = ['Escape', 'Tab'];

const focusById = (id: string) => document.getElementById(id)?.focus();

function jumpTarget(key: string, count: number): number | undefined {
    if (key === 'Home') return 0;
    if (key === 'End') return count - 1;
    return undefined;
}

function handleListKey(event: KeyboardEvent, ctx: KeyContext) {
    const step = KEY_STEPS[event.key];
    const jump = jumpTarget(event.key, ctx.count);
    if (CLOSE_KEYS.includes(event.key)) return ctx.close();
    event.preventDefault();
    if (step !== undefined) ctx.setActiveIndex((ctx.activeIndex + step + ctx.count) % ctx.count);
    else if (jump !== undefined) ctx.setActiveIndex(jump);
    else if (CHOOSE_KEYS.includes(event.key)) ctx.choose(ctx.activeIndex);
}

function useOutsideClose(isOpen: boolean, onClose: () => void) {
    useEffect(() => {
        if (!isOpen) return undefined;
        const handle = (event: MouseEvent) => {
            if (!document.getElementById(PERSONA_ELEMENT_IDS.root)?.contains(event.target as Node)) onClose();
        };
        document.addEventListener('mousedown', handle);
        return () => document.removeEventListener('mousedown', handle);
    }, [isOpen, onClose]);
}

export function usePersonaMenu({ count, selectedIndex, onSelect }: MenuOptions) {
    const [isOpen, setIsOpen] = useState(false);
    const [activeIndex, setActiveIndex] = useState(selectedIndex);
    const dismiss = useCallback(() => setIsOpen(false), []);
    useOutsideClose(isOpen, dismiss);
    useEffect(() => { if (isOpen) focusById(PERSONA_ELEMENT_IDS.list); }, [isOpen]);

    const close = () => { setIsOpen(false); focusById(PERSONA_ELEMENT_IDS.trigger); };
    const toggle = () => { setActiveIndex(selectedIndex); setIsOpen(open => !open); };
    const choose = (index: number) => { onSelect(index); close(); };
    const onKeyDown = (event: KeyboardEvent) => handleListKey(event, { count, activeIndex, setActiveIndex, choose, close });
    return { isOpen, activeIndex, setActiveIndex, toggle, choose, onKeyDown };
}

export type PersonaMenu = ReturnType<typeof usePersonaMenu>;
