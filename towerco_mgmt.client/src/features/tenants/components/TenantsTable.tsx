import { useNavigate } from 'react-router-dom';
import { Pagination, SeverityPill } from '@/shared/components';
import { usePagination } from '@/shared/hooks/usePagination';
import { OPEN_ENDED_LABEL } from '@/shared/constants/domain';
import type { TenantRecord } from '@/features/tenants/types';
import { formatMoney } from '@/shared/utils/format';
import { sitePath } from '@/shared/constants/routes';

function TenantRow({ tenant }: { tenant: TenantRecord }) {
    const navigate = useNavigate();
    return (
        <tr className="clickable" onClick={() => navigate(sitePath(tenant.siteId))}>
            <td>{tenant.operatorName}</td>
            <td><span className="mono">{tenant.siteCode}</span> <span className="muted">{tenant.siteName}</span></td>
            <td>{tenant.countryCode}</td>
            <td className="mono">{tenant.startDate}</td>
            <td className="mono">{tenant.endDate ?? OPEN_ENDED_LABEL}</td>
            <td>{formatMoney(tenant.monthlyRent, tenant.currency)}</td>
            <td>{tenant.equipmentCount}</td>
            <td><SeverityPill severity={tenant.isActive ? 'ok' : 'neutral'} label={tenant.isActive ? 'Active' : 'Ended'} /></td>
        </tr>
    );
}

function TenantsTable({ tenants }: { tenants: TenantRecord[] }) {
    const paging = usePagination(tenants);
    return (
        <>
        <table className="table">
            <thead>
                <tr><th>Operator</th><th>Site</th><th>Country</th><th>Start</th><th>End</th><th>Monthly rent</th><th>Equipment</th><th>Status</th></tr>
            </thead>
            <tbody>{paging.pageRows.map(t => <TenantRow key={t.tenancyId} tenant={t} />)}</tbody>
        </table>
        <Pagination {...paging} />
        </>
    );
}

export default TenantsTable;
