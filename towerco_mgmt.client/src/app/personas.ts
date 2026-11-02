import { ROUTES } from '@/shared/constants/routes';

export type PersonaId = 'viewer' | 'commercial' | 'operations' | 'manager' | 'admin';

export interface Persona {
    id: PersonaId;
    label: string;
    description: string;
    icon: string;
    landingRoute: string;
}

export const ADMIN_PERSONA: PersonaId = 'admin';

export const PERSONAS: readonly Persona[] = [
    { id: 'viewer', label: 'Any user', description: 'Search sites and view details', icon: '🔍', landingRoute: ROUTES.sites },
    { id: 'commercial', label: 'Commercial', description: 'Tenants, operators and capacity', icon: '💼', landingRoute: ROUTES.tenants },
    { id: 'operations', label: 'Operations', description: 'Register sites, equipment and work orders', icon: '🏗️', landingRoute: ROUTES.sites },
    { id: 'manager', label: 'Manager', description: 'Portfolio overview, health and work orders', icon: '📈', landingRoute: ROUTES.dashboard },
    { id: ADMIN_PERSONA, label: 'Admin', description: 'Full access to every page', icon: '🛡️', landingRoute: ROUTES.dashboard },
];

export const DEFAULT_PERSONA: PersonaId = 'viewer';
export const PERSONA_STORAGE_KEY = 'towerco.persona';
export const PERSONA_SWITCHER_LABEL = 'Viewing as';

export const formatPageAccess = (count: number) => (count === 1 ? '1 page' : `${count} pages`);

export const isPersonaId = (value: unknown): value is PersonaId =>
    PERSONAS.some(persona => persona.id === value);

export const getPersona = (id: PersonaId): Persona =>
    PERSONAS.find(persona => persona.id === id) ?? PERSONAS[0];
