import { Link } from 'react-router-dom';
import type { SellableTower } from '@/features/dashboard/types';
import { sitePath } from '@/shared/constants/routes';

function SellableTowersTable({ rows }: { rows: SellableTower[] }) {
    if (!rows.length) return <p className="muted">No active towers with free slots.</p>;
    return (
        <table className="table">
            <thead><tr><th>Site</th><th>Name</th><th>Country</th><th>Free slots</th></tr></thead>
            <tbody>{rows.map(r => (
                <tr key={r.siteId}>
                    <td><Link className="mono" to={sitePath(r.siteId)}>{r.siteCode}</Link></td>
                    <td>{r.siteName}</td><td>{r.countryCode}</td><td>{r.availableSlots}</td>
                </tr>
            ))}</tbody>
        </table>
    );
}

export default SellableTowersTable;
