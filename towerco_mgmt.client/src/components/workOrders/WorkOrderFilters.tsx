import type { ChangeEvent } from 'react';
import { COUNTRIES } from '@/constants/domain';
import { WORK_ORDER_PRIORITIES, WORK_ORDER_STATUSES, WORK_ORDER_STATUS_LABELS } from '@/constants/workOrders';
import type { WorkOrderFilter } from '@/types';

interface WorkOrderFiltersProps {
    value: WorkOrderFilter;
    onChange: (filter: WorkOrderFilter) => void;
}

function WorkOrderFilters({ value, onChange }: WorkOrderFiltersProps) {
    const set = (key: keyof WorkOrderFilter) => (e: ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
        onChange({ ...value, [key]: e.target.value });

    return (
        <div className="filters">
            <input className="input" placeholder="Search code, title or site" value={value.search ?? ''} onChange={set('search')} />
            <select className="input" value={value.status ?? ''} onChange={set('status')}>
                <option value="">All statuses</option>
                {WORK_ORDER_STATUSES.map(s => <option key={s} value={s}>{WORK_ORDER_STATUS_LABELS[s]}</option>)}
            </select>
            <select className="input" value={value.priority ?? ''} onChange={set('priority')}>
                <option value="">All priorities</option>
                {WORK_ORDER_PRIORITIES.map(p => <option key={p} value={p}>{p}</option>)}
            </select>
            <select className="input" value={value.country ?? ''} onChange={set('country')}>
                <option value="">All countries</option>
                {COUNTRIES.map(c => <option key={c.code} value={c.code}>{c.name}</option>)}
            </select>
        </div>
    );
}

export default WorkOrderFilters;
