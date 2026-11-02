import { useNavigate } from 'react-router-dom';
import { CapacityBar, StatusBadge } from '@/components/common';
import type { SiteSummary } from '@/types';

function SitesTable({ sites }: { sites: SiteSummary[] }) {
    const navigate = useNavigate();
    return (
        <table className="table">
            <thead>
                <tr><th>Code</th><th>Name</th><th>Country</th><th>City</th><th>Status</th><th>Tenants</th></tr>
            </thead>
            <tbody>
                {sites.map(site => (
                    <tr key={site.id} className="clickable" onClick={() => navigate(`/sites/${site.id}`)}>
                        <td className="mono">{site.siteCode}</td>
                        <td>{site.name}</td>
                        <td>{site.countryCode}</td>
                        <td>{site.city}</td>
                        <td><StatusBadge status={site.status} /></td>
                        <td><CapacityBar capacity={site.capacity} /></td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
}

export default SitesTable;
