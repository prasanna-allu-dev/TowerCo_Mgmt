import type { NavGroup, NavItem } from '@/app/navigation';
import { ADMIN_PERSONA } from '@/app/personas';
import type { PersonaId } from '@/app/personas';

const isVisibleTo = (item: NavItem, persona: PersonaId) =>
    persona === ADMIN_PERSONA || !item.personas || item.personas.includes(persona);

export function filterNavGroups(groups: NavGroup[], persona: PersonaId): NavGroup[] {
    return groups
        .map(group => ({ ...group, items: group.items.filter(item => isVisibleTo(item, persona)) }))
        .filter(group => group.items.length > 0);
}

export const countNavItems = (groups: NavGroup[], persona: PersonaId): number =>
    filterNavGroups(groups, persona).reduce((total, group) => total + group.items.length, 0);
