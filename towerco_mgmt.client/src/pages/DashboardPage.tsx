import { dashboardApi } from '@/api';
import { Card, StateMessage } from '@/components/common';
import {
    AlertsPanel, BarList, ExpiringTenanciesTable, KpiRow, RentList, SellableTowersTable,
    countryBars, operatorBars, statusBars,
} from '@/components/dashboard';
import { RENEWAL_WINDOW_DAYS } from '@/constants/domain';
import { useApi } from '@/hooks/useApi';
import PageHeader from '@/layout/PageHeader';
import type { PortfolioDashboard } from '@/types';

const PERCENT_CEILING = 100;

function DashboardBody({ data }: { data: PortfolioDashboard }) {
    return (
        <>
            <KpiRow kpis={data.kpis} alerts={data.alerts} />
            <Card title="Critical indicators"><AlertsPanel alerts={data.alerts} /></Card>
            <div className="grid-2">
                <Card title="Utilisation by country"><BarList items={countryBars(data)} max={PERCENT_CEILING} /></Card>
                <Card title="Sites by status"><BarList items={statusBars(data)} /></Card>
                <Card title="Active tenancies by operator"><BarList items={operatorBars(data)} /></Card>
                <Card title="Monthly rent (active, per currency)"><RentList rows={data.monthlyRentByCurrency} /></Card>
            </div>
            <div className="grid-2">
                <Card title={`Renewals due in ${RENEWAL_WINDOW_DAYS} days`}><ExpiringTenanciesTable rows={data.expiringTenancies} /></Card>
                <Card title="Sellable capacity (active sites)"><SellableTowersTable rows={data.sellableTowers} /></Card>
            </div>
        </>
    );
}

function DashboardPage() {
    const { data, error, loading } = useApi(() => dashboardApi.get());
    return (
        <>
            <PageHeader title="Portfolio dashboard" subtitle="Super admin overview of sites, capacity and commercial position" />
            <StateMessage loading={loading} error={error} />
            {data && <DashboardBody data={data} />}
        </>
    );
}

export default DashboardPage;
