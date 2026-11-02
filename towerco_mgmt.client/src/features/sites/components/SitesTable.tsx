import { useNavigate } from 'react-router-dom';
import { CapacityBar, Pagination, StatusBadge } from '@/shared/components';
import type { SiteSummary } from '@/features/sites/types';
import { sitePath } from '@/shared/constants/routes';
import { usePagination } from '@/shared/hooks/usePagination';

function SitesTable({ sites }: { sites: SiteSummary[] }) {
    const navigate = useNavigate();
    const paging = usePagination(sites);
    return (
        <>
        <table className="table">
            <thead>
                <tr><th>Code</th><th>Name</th><th>Country</th><th>City</th><th>Status</th><th>Tenants</th></tr>
            </thead>
            <tbody>
                {paging.pageRows.map(site => (
                    <tr key={site.id} className="clickable" onClick={() => navigate(sitePath(site.id))}>
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
        <Pagination {...paging} />
        </>
    );
}

export default SitesTable;
