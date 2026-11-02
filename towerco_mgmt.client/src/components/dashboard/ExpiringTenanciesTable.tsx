import { Link } from 'react-router-dom';
import { RENEWAL_WINDOW_DAYS } from '@/constants/domain';
import type { ExpiringTenancy } from '@/types';
import { renewalSeverity } from '@/utils/severity';
import SeverityPill from './SeverityPill';

function ExpiringTenanciesTable({ rows }: { rows: ExpiringTenancy[] }) {
    if (!rows.length) return <p className="muted">No tenancies end in the next {RENEWAL_WINDOW_DAYS} days.</p>;
    return (
        <table className="table">
            <thead><tr><th>Site</th><th>Operator</th><th>Ends</th><th>Days left</th></tr></thead>
            <tbody>{rows.map(r => (
                <tr key={r.tenancyId}>
                    <td><Link className="mono" to={`/sites/${r.siteId}`}>{r.siteCode}</Link></td>
                    <td>{r.operatorName}</td><td>{r.endDate}</td>
                    <td><SeverityPill severity={renewalSeverity(r.daysRemaining)} label={`${r.daysRemaining} days`} /></td>
                </tr>
            ))}</tbody>
        </table>
    );
}

export default ExpiringTenanciesTable;
