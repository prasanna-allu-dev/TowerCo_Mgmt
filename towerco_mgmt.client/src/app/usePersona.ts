import { useContext } from 'react';
import { PersonaContext } from '@/app/personaContext';

export const usePersona = () => useContext(PersonaContext);
