import { useCallback, useMemo, useState } from 'react';
import type { ReactNode } from 'react';
import { PersonaContext } from '@/app/personaContext';
import { DEFAULT_PERSONA, PERSONA_STORAGE_KEY, isPersonaId } from '@/app/personas';
import type { PersonaId } from '@/app/personas';

function readStoredPersona(): PersonaId {
    try {
        const stored = localStorage.getItem(PERSONA_STORAGE_KEY);
        return isPersonaId(stored) ? stored : DEFAULT_PERSONA;
    } catch {
        return DEFAULT_PERSONA;
    }
}

function storePersona(id: PersonaId) {
    try {
        localStorage.setItem(PERSONA_STORAGE_KEY, id);
    } catch {
        // Storage blocked: persona still works for this page load.
    }
}

function PersonaProvider({ children }: { children: ReactNode }) {
    const [persona, setPersonaState] = useState<PersonaId>(readStoredPersona);

    const setPersona = useCallback((id: PersonaId) => {
        setPersonaState(id);
        storePersona(id);
    }, []);

    const value = useMemo(() => ({ persona, setPersona }), [persona, setPersona]);
    return <PersonaContext.Provider value={value}>{children}</PersonaContext.Provider>;
}

export default PersonaProvider;
