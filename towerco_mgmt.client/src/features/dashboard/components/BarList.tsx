import type { Severity } from '@/shared/types/domain';

export interface BarItem {
    key: string;
    label: string;
    value: number;
    display?: string;
    tone?: Severity;
}

const PERCENT = 100;

function BarList({ items, max }: { items: BarItem[]; max?: number }) {
    const ceiling = max ?? Math.max(1, ...items.map(i => i.value));
    return (
        <ul className="bar-list">
            {items.map(item => (
                <li key={item.key}>
                    <span className="bar-label">{item.label}</span>
                    <span className="bar-track"><span className={`bar-fill tone-${item.tone ?? 'neutral'}`} style={{ width: `${(item.value / ceiling) * PERCENT}%` }} /></span>
                    <span className="bar-value">{item.display ?? item.value}</span>
                </li>
            ))}
        </ul>
    );
}

export default BarList;
