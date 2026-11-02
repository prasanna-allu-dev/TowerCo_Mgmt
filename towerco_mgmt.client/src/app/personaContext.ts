import { createContext } from 'react';
import { DEFAULT_PERSONA } from '@/app/personas';
import type { PersonaId } from '@/app/personas';

export interface PersonaContextValue {
    persona: PersonaId;
    setPersona: (id: PersonaId) => void;
}

export const PersonaContext = createContext<PersonaContextValue>({
    persona: DEFAULT_PERSONA,
    setPersona: () => undefined,
});
