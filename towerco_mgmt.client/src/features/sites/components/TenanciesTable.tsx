import { OPEN_ENDED_LABEL } from '@/shared/constants/domain';
import type { Tenancy } from '@/features/tenants/types';
import { formatMoney } from '@/shared/utils/format';

const formatRent = (tenancy: Tenancy) => formatMoney(tenancy.monthlyRent, tenancy.currency);

function TenanciesTable({ tenancies }: { tenancies: Tenancy[] }) {
    return (
        <table className="table">
            <thead>
                <tr><th>Operator</th><th>Start</th><th>End</th><th>Monthly rent</th></tr>
            </thead>
            <tbody>
                {tenancies.map(t => (
                    <tr key={t.id}>
                        <td>{t.operatorName}</td>
                        <td className="mono">{t.startDate}</td>
                        <td className="mono">{t.endDate ?? OPEN_ENDED_LABEL}</td>
                        <td>{formatRent(t)}</td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
}

export default TenanciesTable;
