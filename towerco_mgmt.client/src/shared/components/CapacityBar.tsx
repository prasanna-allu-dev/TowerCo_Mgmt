import { UTILISATION_WARNING_PERCENT } from '@/shared/constants/domain';
import type { CapacitySummary } from '@/shared/types/domain';

function barLevel(capacity: CapacitySummary): string {
    if (!capacity.hasCapacity) return 'full';
    return capacity.utilisationPercent >= UTILISATION_WARNING_PERCENT ? 'high' : 'ok';
}

function CapacityBar({ capacity }: { capacity: CapacitySummary }) {
    return (
        <div className="capacity">
            <div className="capacity-track">
                <div className={`capacity-fill ${barLevel(capacity)}`} style={{ width: `${capacity.utilisationPercent}%` }} />
            </div>
            <span className="capacity-label">{capacity.activeTenantCount}/{capacity.maxTenants}</span>
        </div>
    );
}

export default CapacityBar;
