import type { ChangeEvent } from 'react';
import { COUNTRIES, SITE_STATUSES } from '@/constants/domain';
import type { SiteFilter } from '@/types';

interface SiteFiltersProps {
    value: SiteFilter;
    onChange: (filter: SiteFilter) => void;
}

function SiteFilters({ value, onChange }: SiteFiltersProps) {
    const set = (key: keyof SiteFilter) => (e: ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
        onChange({ ...value, [key]: e.target.value });

    return (
        <div className="filters">
            <input className="input" placeholder="Search code, name or city" value={value.search ?? ''} onChange={set('search')} />
            <select className="input" value={value.country ?? ''} onChange={set('country')}>
                <option value="">All countries</option>
                {COUNTRIES.map(c => <option key={c.code} value={c.code}>{c.name}</option>)}
            </select>
            <select className="input" value={value.status ?? ''} onChange={set('status')}>
                <option value="">All statuses</option>
                {SITE_STATUSES.map(s => <option key={s} value={s}>{s}</option>)}
            </select>
        </div>
    );
}

export default SiteFilters;
