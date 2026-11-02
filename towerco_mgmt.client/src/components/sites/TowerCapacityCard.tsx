import { CapacityBar, Card } from '@/components/common';
import type { SiteDetail } from '@/types';

function TowerCapacityCard({ site }: { site: SiteDetail }) {
    const { tower, capacity } = site;
    return (
        <Card title="Tower and capacity">
            <dl className="details">
                <dt>Type</dt><dd>{tower.towerType}</dd>
                <dt>Height</dt><dd>{tower.heightMetres} m</dd>
                <dt>Slots</dt><dd>{capacity.activeTenantCount} of {capacity.maxTenants} used, {capacity.utilisationPercent}%</dd>
            </dl>
            <CapacityBar capacity={capacity} />
            {!capacity.hasCapacity && <p className="muted">No capacity for more tenants.</p>}
        </Card>
    );
}

export default TowerCapacityCard;
