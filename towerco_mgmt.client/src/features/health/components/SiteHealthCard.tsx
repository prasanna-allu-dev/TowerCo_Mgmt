import { Card } from '@/shared/components';
import { INSPECTION_OUTCOME_LABELS, SENSOR_LABELS } from '@/features/health/constants/health';
import type { SensorSeries, SiteHealth } from '@/features/health/types';
import HealthPill from './HealthPill';
import Sparkline from './Sparkline';

function SensorTile({ series }: { series: SensorSeries }) {
    const { label, unit } = SENSOR_LABELS[series.sensorType];
    return (
        <div className="sensor-tile">
            <div className="sensor-head"><span>{label}</span><HealthPill status={series.status} /></div>
            <div className="kpi-value">{series.latest} {unit}</div>
            <Sparkline points={series.points} warning={series.warning} critical={series.critical} />
            <div className="muted">Warning {series.warning} {unit}, critical {series.critical} {unit}</div>
        </div>
    );
}

function SiteHealthCard({ health }: { health: SiteHealth }) {
    const last = health.inspections[0];
    return (
        <Card title="Tower health">
            <div className="sensor-head"><HealthPill status={health.summary.overall} /><span>{health.summary.mainReason}</span></div>
            <ul className="signal-list">
                {health.summary.signals.map(s => <li key={s.name}><HealthPill status={s.status} /> <strong>{s.name}</strong>: {s.reason}</li>)}
            </ul>
            {health.telemetry.length > 0
                ? <div className="sensor-grid">{health.telemetry.map(s => <SensorTile key={s.sensorType} series={s} />)}</div>
                : <p className="muted">No telemetry for this site in the last 30 days.</p>}
            {last && <p className="muted">Last inspection {last.inspectedOn} by {last.inspector}: {INSPECTION_OUTCOME_LABELS[last.outcome]}</p>}
        </Card>
    );
}

export default SiteHealthCard;
