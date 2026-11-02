import { useState } from 'react';
import { lifecycleApi } from '@/features/sites/api';
import type { SiteDetail } from '@/features/sites/types';
import { TOWER_LIMITS } from '@/shared/constants/domain';
import { useAsyncAction } from '@/shared/hooks/useAsyncAction';

interface CapacityEditorProps {
    site: SiteDetail;
    onChanged: () => void;
}

/** Structural upgrade or downgrade. The API refuses to go below slots already committed to tenants. */
function CapacityEditor({ site, onChanged }: CapacityEditorProps) {
    const [maxTenants, setMaxTenants] = useState(site.capacity.maxTenants);
    const save = useAsyncAction(lifecycleApi.changeCapacity, onChanged);

    return (
        <div className="action-row">
            <label className="muted" htmlFor="max-tenants">Tenant slots</label>
            <input id="max-tenants" className="input action-input" type="number" value={maxTenants}
                min={TOWER_LIMITS.minTenants} max={TOWER_LIMITS.maxTenants} onChange={e => setMaxTenants(Number(e.target.value))} />
            <button className="action-btn primary" disabled={save.pending || maxTenants === site.capacity.maxTenants}
                onClick={() => save.run(site.tower.id, maxTenants)}>Change capacity</button>
            {save.error && <p className="alert-error">{save.error}</p>}
        </div>
    );
}

export default CapacityEditor;
