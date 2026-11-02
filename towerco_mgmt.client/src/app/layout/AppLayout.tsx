import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import TopBar from './TopBar';

function AppLayout() {
    return (
        <div className="shell">
            <Sidebar />
            <main className="content">
                <TopBar />
                <Outlet />
            </main>
        </div>
    );
}

export default AppLayout;
