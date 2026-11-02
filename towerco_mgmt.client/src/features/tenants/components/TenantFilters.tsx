import type { ChangeEvent } from 'react';
import { COUNTRIES, TENANCY_STATES } from '@/shared/constants/domain';
import type { Operator } from '@/features/operators/types';
import type { TenantFilter } from '@/features/tenants/types';

interface TenantFiltersProps {
    value: TenantFilter;
    operators: Operator[];
    onChange: (filter: TenantFilter) => void;
}

function TenantFilters({ value, operators, onChange }: TenantFiltersProps) {
    const set = (key: keyof TenantFilter) => (e: ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
        onChange({ ...value, [key]: e.target.value });

    return (
        <div className="filters">
            <input className="input" placeholder="Search operator, site code or site name" value={value.search ?? ''} onChange={set('search')} />
            <select className="input" value={value.operatorId ?? ''} onChange={set('operatorId')}>
                <option value="">All operators</option>
                {operators.map(o => <option key={o.id} value={o.id}>{o.name}</option>)}
            </select>
            <select className="input" value={value.country ?? ''} onChange={set('country')}>
                <option value="">All countries</option>
                {COUNTRIES.map(c => <option key={c.code} value={c.code}>{c.name}</option>)}
            </select>
            <select className="input" value={value.state ?? ''} onChange={set('state')}>
                <option value="">All tenancies</option>
                {TENANCY_STATES.map(s => <option key={s.value} value={s.value}>{s.label}</option>)}
            </select>
        </div>
    );
}

export default TenantFilters;
