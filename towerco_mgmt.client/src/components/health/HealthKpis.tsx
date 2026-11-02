import { KpiTile } from '@/components/dashboard';
import type { PortfolioHealth } from '@/types';
import { countSeverity } from '@/utils/severity';

function HealthKpis({ health }: { health: PortfolioHealth }) {
    const total = health.healthy + health.warning + health.critical;
    return (
        <div className="kpi-row">
            <KpiTile label="Critical" value={health.critical} hint="need action now" tone={countSeverity(health.critical, 'critical')} />
            <KpiTile label="Warning" value={health.warning} hint="watch closely" tone={countSeverity(health.warning, 'warning')} />
            <KpiTile label="Healthy" value={health.healthy} hint="all signals clear" tone="ok" />
            <KpiTile label="Sites scored" value={total} hint="telemetry, inspection, work orders" />
        </div>
    );
}

export default HealthKpis;
