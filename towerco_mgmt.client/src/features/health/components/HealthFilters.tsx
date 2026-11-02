import type { ChangeEvent } from 'react';
import { COUNTRIES } from '@/shared/constants/domain';
import { HEALTH_STATUSES } from '@/features/health/constants/health';
import type { HealthFilter } from '@/features/health/types';

interface HealthFiltersProps {
    value: HealthFilter;
    onChange: (filter: HealthFilter) => void;
}

function HealthFilters({ value, onChange }: HealthFiltersProps) {
    const set = (key: keyof HealthFilter) => (e: ChangeEvent<HTMLSelectElement>) => onChange({ ...value, [key]: e.target.value });

    return (
        <div className="filters">
            <select className="input" value={value.country ?? ''} onChange={set('country')}>
                <option value="">All countries</option>
                {COUNTRIES.map(c => <option key={c.code} value={c.code}>{c.name}</option>)}
            </select>
            <select className="input" value={value.status ?? ''} onChange={set('status')}>
                <option value="">All health levels</option>
                {HEALTH_STATUSES.map(s => <option key={s} value={s}>{s}</option>)}
            </select>
        </div>
    );
}

export default HealthFilters;
