import { ROUTES } from '@/shared/constants/routes';
import type { PersonaId } from '@/app/personas';

export const APP_NAME = 'TowerCo';
export const APP_SUBTITLE = 'Tower Management';


export interface NavItem {
    label: string;
    path: string;
    icon: string;
    disabled?: boolean;
    /** Personas that see this item. Omitted means every persona. */
    personas?: PersonaId[];
}

export interface NavGroup {
    label: string;
    items: NavItem[];
}

export const NAV_GROUPS: NavGroup[] = [
    { label: 'Overview', items: [{ label: 'Dashboard', path: ROUTES.dashboard, icon: '📊', personas: ['commercial', 'manager'] }] },
    {
        label: 'Portfolio',
        items: [
            { label: 'Sites', path: ROUTES.sites, icon: '📡' },
            { label: 'Configuration', path: ROUTES.configuration, icon: '⚙️', personas: ['operations'] },
            { label: 'Equipment', path: ROUTES.equipment, icon: '🔧', personas: ['operations'] },
        ],
    },
    {
        label: 'Commercial',
        items: [
            { label: 'Tenants', path: ROUTES.tenants, icon: '🏢', personas: ['commercial', 'manager'] },
            { label: 'Operators', path: ROUTES.operators, icon: '📶', personas: ['commercial'] },
        ],
    },
    {
        label: 'Operations',
        items: [
            { label: 'Tower health', path: ROUTES.health, icon: '🩺', personas: ['operations', 'manager'] },
            { label: 'Work orders', path: ROUTES.workOrders, icon: '🛠️', personas: ['operations', 'manager'] },
        ],
    },
];
