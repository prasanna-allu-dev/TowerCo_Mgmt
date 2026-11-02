import type { ReactNode } from 'react';

interface PageHeaderProps {
    title: string;
    subtitle?: string;
    actions?: ReactNode;
}

function PageHeader({ title, subtitle, actions }: PageHeaderProps) {
    return (
        <header className="page-header">
            <div>
                <h1>{title}</h1>
                {subtitle && <p className="muted">{subtitle}</p>}
            </div>
            {actions && <div className="page-actions">{actions}</div>}
        </header>
    );
}

export default PageHeader;
