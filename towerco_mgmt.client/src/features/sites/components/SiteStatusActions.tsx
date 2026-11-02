import { lifecycleApi } from '@/features/sites/api';
import { DECOMMISSIONED, SITE_STATUS_ACTION_LABELS, SITE_TRANSITIONS } from '@/features/sites/constants/lifecycle';
import type { SiteDetail } from '@/features/sites/types';
import { useAsyncAction } from '@/shared/hooks/useAsyncAction';

interface SiteStatusActionsProps {
    site: SiteDetail;
    onChanged: () => void;
}

/** Next lifecycle steps for the site. The API rejects activation without a tower and decommissioning with tenants. */
function SiteStatusActions({ site, onChanged }: SiteStatusActionsProps) {
    const save = useAsyncAction(lifecycleApi.changeStatus, onChanged);
    const next = SITE_TRANSITIONS[site.status];

    if (next.length === 0) return <p className="muted">Decommissioned. No further lifecycle steps.</p>;
    return (
        <div className="action-row">
            {next.map(status => (
                <button key={status} className={`action-btn ${status === DECOMMISSIONED ? 'danger' : 'primary'}`}
                    disabled={save.pending} onClick={() => save.run(site.id, status)}>{SITE_STATUS_ACTION_LABELS[status]}</button>
            ))}
            {save.error && <p className="alert-error">{save.error}</p>}
        </div>
    );
}

export default SiteStatusActions;
