import type { DashboardAlert, PortfolioKpis } from '@/types';
import { countSeverity, utilisationSeverity } from '@/utils/severity';
import KpiTile from './KpiTile';

interface KpiRowProps {
    kpis: PortfolioKpis;
    alerts: DashboardAlert[];
}

function KpiRow({ kpis, alerts }: KpiRowProps) {
    const critical = alerts.filter(a => a.severity === 'critical').length;
    const freeSlots = kpis.totalSlots - kpis.occupiedSlots;
    return (
        <div className="kpi-row">
            <KpiTile label="Critical alerts" value={critical} hint={`${alerts.length} alerts in total`} tone={countSeverity(critical, 'critical')} />
            <KpiTile label="Utilisation" value={`${kpis.utilisationPercent}%`} hint={`${kpis.occupiedSlots} of ${kpis.totalSlots} slots`} tone={utilisationSeverity(kpis.utilisationPercent)} />
            <KpiTile label="Free slots" value={freeSlots} hint="across all towers" tone={freeSlots > 0 ? 'ok' : 'critical'} />
            <KpiTile label="Full towers" value={kpis.fullTowers} hint="no capacity left" tone={countSeverity(kpis.fullTowers, 'critical')} />
            <KpiTile label="Empty towers" value={kpis.emptyTowers} hint="no tenants" tone={countSeverity(kpis.emptyTowers, 'warning')} />
            <KpiTile label="Sites" value={kpis.totalSites} hint={`${kpis.activeSites} active`} />
        </div>
    );
}

export default KpiRow;
