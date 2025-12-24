import { NavLink, Outlet } from "react-router-dom";
import "./AppLayout.css";

export function AppLayout() {
  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brandTitle">FinancialManager</div>
        </div>

        <nav className="nav">
          <NavLink className={({ isActive }) => `navItem ${isActive ? "active" : ""}`} to="/">
            Dashboard
          </NavLink>
          <NavLink className={({ isActive }) => `navItem ${isActive ? "active" : ""}`} to="/pessoas">
            Pessoas
          </NavLink>
          <NavLink className={({ isActive }) => `navItem ${isActive ? "active" : ""}`} to="/categorias">
            Categorias
          </NavLink>
          <NavLink className={({ isActive }) => `navItem ${isActive ? "active" : ""}`} to="/transacoes">
            Transações
          </NavLink>
        </nav>
      </aside>

      <main className="main">
        <header className="topbar">
          <div className="breadcrumb">
            <span className="muted">Dashboard</span>
            <span className="sep">/</span>
            <span>Totais por pessoa</span>
          </div>
        </header>

        <section className="content">
          <div className="contentInner">
            <Outlet />
          </div>
        </section>
      </main>
    </div>
  );
}
