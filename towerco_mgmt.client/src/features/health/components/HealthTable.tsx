import { Link } from 'react-router-dom';
import type { SiteHealthSummary } from '@/features/health/types';
import HealthPill from './HealthPill';
import { sitePath } from '@/shared/constants/routes';

function HealthTable({ sites }: { sites: SiteHealthSummary[] }) {
    return (
        <table className="table">
            <thead>
                <tr><th>Site</th><th>Country</th><th>Health</th><th>Main reason</th>{sites[0]?.signals.map(s => <th key={s.name}>{s.name}</th>)}</tr>
            </thead>
            <tbody>
                {sites.map(site => (
                    <tr key={site.siteId}>
                        <td><Link to={sitePath(site.siteId)}>{site.siteCode}</Link><div className="muted">{site.siteName}</div></td>
                        <td>{site.countryCode}</td>
                        <td><HealthPill status={site.overall} /></td>
                        <td>{site.mainReason}</td>
                        {site.signals.map(s => <td key={s.name} title={s.reason}><HealthPill status={s.status} /></td>)}
                    </tr>
                ))}
            </tbody>
        </table>
    );
}

export default HealthTable;
