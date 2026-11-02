import type { ReactNode } from 'react';
import { OPEN_ENDED_LABEL } from '@/shared/constants/domain';
import type { Tenancy } from '@/features/tenants/types';
import { formatMoney } from '@/shared/utils/format';
import { Pagination } from '@/shared/components';
import { usePagination } from '@/shared/hooks/usePagination';

const formatRent = (tenancy: Tenancy) => formatMoney(tenancy.monthlyRent, tenancy.currency);

interface TenanciesTableProps {
    tenancies: Tenancy[];
    renderActions?: (tenancy: Tenancy) => ReactNode;
}

function TenanciesTable({ tenancies, renderActions }: TenanciesTableProps) {
    const paging = usePagination(tenancies);
    return (
        <>
        <table className="table">
            <thead>
                <tr><th>Operator</th><th>Start</th><th>End</th><th>Monthly rent</th>{renderActions && <th>Actions</th>}</tr>
            </thead>
            <tbody>
                {paging.pageRows.map(t => (
                    <tr key={t.id}>
                        <td>{t.operatorName}</td>
                        <td className="mono">{t.startDate}</td>
                        <td className="mono">{t.endDate ?? OPEN_ENDED_LABEL}</td>
                        <td>{formatRent(t)}</td>
                        {renderActions && <td>{renderActions(t)}</td>}
                    </tr>
                ))}
            </tbody>
        </table>
        <Pagination {...paging} />
        </>
    );
}

export default TenanciesTable;
