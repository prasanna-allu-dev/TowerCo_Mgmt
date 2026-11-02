import type { Severity } from '@/types';

interface KpiTileProps {
    label: string;
    value: string | number;
    hint?: string;
    tone?: Severity;
}

function KpiTile({ label, value, hint, tone = 'neutral' }: KpiTileProps) {
    return (
        <div className={`kpi tone-${tone}`}>
            <div className="kpi-label">{label}</div>
            <div className="kpi-value">{value}</div>
            {hint && <div className="kpi-hint">{hint}</div>}
        </div>
    );
}

export default KpiTile;
