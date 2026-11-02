import { useState } from 'react';

const STORAGE_KEY = 'towerco.sidebarExpanded';
const EXPANDED = 'true';

function readExpanded(): boolean {
    try {
        return localStorage.getItem(STORAGE_KEY) === EXPANDED;
    } catch {
        return false;
    }
}

function saveExpanded(expanded: boolean) {
    try {
        localStorage.setItem(STORAGE_KEY, String(expanded));
    } catch {
        // Storage unavailable (private mode): preference simply is not remembered.
    }
}

/** Collapsed icon rail by default; the choice is remembered per browser. */
export function useSidebarExpanded() {
    const [expanded, setExpanded] = useState(readExpanded);
    const toggle = () => setExpanded(current => {
        saveExpanded(!current);
        return !current;
    });
    return { expanded, toggle };
}
