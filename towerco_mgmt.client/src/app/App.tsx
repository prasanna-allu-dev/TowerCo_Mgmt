import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import AppLayout from './layout/AppLayout';
import { SiteDetailPage, SitesPage } from '@/features/sites';
import { OperatorsPage } from '@/features/operators';
import { ConfigurationPage, CreateTowerSitePage } from '@/features/configuration';
import { DashboardPage } from '@/features/dashboard';
import { TenantsPage } from '@/features/tenants';
import { EquipmentPage } from '@/features/equipment';
import { WorkOrderDetailPage, WorkOrdersPage } from '@/features/workOrders';
import { HealthPage } from '@/features/health';
import { ROUTES } from '@/shared/constants/routes';
import './App.css';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<AppLayout />}>
                    <Route path={ROUTES.sites} element={<SitesPage />} />
                    <Route path={ROUTES.siteDetail} element={<SiteDetailPage />} />
                    <Route path={ROUTES.operators} element={<OperatorsPage />} />
                    <Route path={ROUTES.tenants} element={<TenantsPage />} />
                    <Route path={ROUTES.equipment} element={<EquipmentPage />} />
                    <Route path={ROUTES.dashboard} element={<DashboardPage />} />
                    <Route path={ROUTES.configuration} element={<ConfigurationPage />} />
                    <Route path={ROUTES.newTowerSite} element={<CreateTowerSitePage />} />
                    <Route path={ROUTES.workOrders} element={<WorkOrdersPage />} />
                    <Route path={ROUTES.workOrderDetail} element={<WorkOrderDetailPage />} />
                    <Route path={ROUTES.health} element={<HealthPage />} />
                    <Route path="*" element={<Navigate to={ROUTES.sites} replace />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}

export default App;
