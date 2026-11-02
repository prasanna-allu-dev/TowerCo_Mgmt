import type { ChangeEvent } from 'react';
import { COUNTRIES, EQUIPMENT_CATEGORIES, EQUIPMENT_STATUSES, OWNER_FILTERS } from '@/shared/constants/domain';
import type { EquipmentFilter } from '@/features/equipment/types';

interface EquipmentFiltersProps {
    value: EquipmentFilter;
    onChange: (filter: EquipmentFilter) => void;
}

function EquipmentFilters({ value, onChange }: EquipmentFiltersProps) {
    const set = (key: keyof EquipmentFilter) => (e: ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
        onChange({ ...value, [key]: e.target.value });

    return (
        <div className="filters">
            <input className="input" placeholder="Search serial, type, site or owner" value={value.search ?? ''} onChange={set('search')} />
            <select className="input" value={value.category ?? ''} onChange={set('category')}>
                <option value="">All categories</option>
                {EQUIPMENT_CATEGORIES.map(c => <option key={c} value={c}>{c}</option>)}
            </select>
            <select className="input" value={value.owner ?? ''} onChange={set('owner')}>
                <option value="">All owners</option>
                {OWNER_FILTERS.map(o => <option key={o.value} value={o.value}>{o.label}</option>)}
            </select>
            <select className="input" value={value.status ?? ''} onChange={set('status')}>
                <option value="">All statuses</option>
                {EQUIPMENT_STATUSES.map(s => <option key={s} value={s}>{s}</option>)}
            </select>
            <select className="input" value={value.country ?? ''} onChange={set('country')}>
                <option value="">All countries</option>
                {COUNTRIES.map(c => <option key={c.code} value={c.code}>{c.name}</option>)}
            </select>
        </div>
    );
}

export default EquipmentFilters;
