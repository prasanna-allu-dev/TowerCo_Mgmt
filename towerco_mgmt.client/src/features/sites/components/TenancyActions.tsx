import { useState } from 'react';
import { lifecycleApi } from '@/features/sites/api';
import { TENANCY_ACTIONS, type TenancyAction } from '@/features/sites/constants/lifecycle';
import type { Tenancy } from '@/features/tenants/types';
import { useAsyncAction } from '@/shared/hooks/useAsyncAction';

interface TenancyActionsProps {
    towerId: number;
    tenancy: Tenancy;
    onChanged: () => void;
}

const run = (towerId: number, tenancy: Tenancy, action: TenancyAction, value: string) => {
    if (action === 'end') return lifecycleApi.endTenancy(towerId, tenancy.id, value);
    if (action === 'renew') return lifecycleApi.renewTenancy(towerId, tenancy.id, value || null);
    return lifecycleApi.reviewRent(towerId, tenancy.id, Number(value));
};

/** End, renew or re-price one tenancy. Capacity and term rules are enforced by the Tower aggregate (409 on breach). */
function TenancyActions({ towerId, tenancy, onChanged }: TenancyActionsProps) {
    const [action, setAction] = useState<TenancyAction>('end');
    const [value, setValue] = useState('');
    const save = useAsyncAction(run, onChanged);
    const input = TENANCY_ACTIONS.find(a => a.value === action)?.input ?? 'date';

    return (
        <div className="action-row">
            <select className="input action-input" value={action} onChange={e => { setAction(e.target.value as TenancyAction); setValue(''); }}>
                {TENANCY_ACTIONS.map(a => <option key={a.value} value={a.value}>{a.label}</option>)}
            </select>
            <input className="input action-input" type={input} value={value} aria-label={action} onChange={e => setValue(e.target.value)} />
            <button className="action-btn primary" disabled={save.pending || (!value && action !== 'renew')}
                onClick={() => save.run(towerId, tenancy, action, value)}>Apply</button>
            {save.error && <p className="alert-error">{save.error}</p>}
        </div>
    );
}

export default TenancyActions;
