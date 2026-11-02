import { SPARKLINE } from '@/features/health/constants/health';
import type { ReadingPoint } from '@/features/health/types';

interface SparklineProps {
    points: ReadingPoint[];
    warning: number;
    critical: number;
}

/** 30-day trend with dashed warning and critical limit lines. */
function Sparkline({ points, warning, critical }: SparklineProps) {
    const { width, height, padding } = SPARKLINE;
    const values = points.map(p => p.value);
    const min = Math.min(...values, warning, critical);
    const max = Math.max(...values, warning, critical);
    const y = (v: number) => height - padding - ((v - min) / (max - min || 1)) * (height - 2 * padding);
    const x = (i: number) => padding + (i / Math.max(points.length - 1, 1)) * (width - 2 * padding);
    const path = values.map((v, i) => `${i ? 'L' : 'M'}${x(i).toFixed(1)},${y(v).toFixed(1)}`).join(' ');

    return (
        <svg className="sparkline" viewBox={`0 0 ${width} ${height}`} role="img" aria-label="30-day trend">
            <line className="limit tone-warning" x1={0} x2={width} y1={y(warning)} y2={y(warning)} />
            <line className="limit tone-critical" x1={0} x2={width} y1={y(critical)} y2={y(critical)} />
            <path d={path} className="trend" />
        </svg>
    );
}

export default Sparkline;
