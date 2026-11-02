import KpiTile from '@/components/dashboard/KpiTile';
import type { TenantRecord } from '@/types';

function TenantKpis({ tenants }: { tenants: TenantRecord[] }) {
    const active = tenants.filter(t => t.isActive);
    const operators = new Set(active.map(t => t.operatorId)).size;
    const equipment = active.reduce((sum, t) => sum + t.equipmentCount, 0);
    return (
        <div className="kpi-row">
            <KpiTile label="Active tenancies" value={active.length} hint={`${tenants.length - active.length} ended`} tone="ok" />
            <KpiTile label="Operators" value={operators} hint="with an active tenancy" />
            <KpiTile label="Tenant equipment" value={equipment} hint="installed items" />
        </div>
    );
}

export default TenantKpis;
