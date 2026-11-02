import { ROUTES } from '@/shared/constants/routes';

export const APP_NAME = 'TowerCo';
export const APP_SUBTITLE = 'Tower Management';


export interface NavItem {
    label: string;
    path: string;
    icon: string;
    disabled?: boolean;
}

export interface NavGroup {
    label: string;
    items: NavItem[];
}

export const NAV_GROUPS: NavGroup[] = [
    { label: 'Overview', items: [{ label: 'Dashboard', path: ROUTES.dashboard, icon: '📊' }] },
    {
        label: 'Portfolio',
        items: [
            { label: 'Sites', path: ROUTES.sites, icon: '📡' },
            { label: 'Configuration', path: ROUTES.configuration, icon: '⚙️' },
            { label: 'Equipment', path: ROUTES.equipment, icon: '🔧' },
        ],
    },
    {
        label: 'Commercial',
        items: [
            { label: 'Tenants', path: ROUTES.tenants, icon: '🏢' },
            { label: 'Operators', path: ROUTES.operators, icon: '📶' },
        ],
    },
    {
        label: 'Operations',
        items: [
            { label: 'Tower health', path: ROUTES.health, icon: '🩺' },
            { label: 'Work orders', path: ROUTES.workOrders, icon: '🛠️' },
        ],
    },
];
