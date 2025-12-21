import { NavLink } from "react-router-dom";

const linkBase: React.CSSProperties = {
  display: "block",
  padding: "10px 12px",
  borderRadius: 10,
  textDecoration: "none",
  color: "#111",
};

export default function Sidebar() {
  return (
    <aside style={{ width: 240, padding: 16, borderRight: "1px solid #eee" }}>
      <div style={{ fontWeight: 800, marginBottom: 12 }}>FinancialManager</div>

      <nav style={{ display: "grid", gap: 8 }}>
        <NavLink
          to="/"
          end
          style={({ isActive }) => ({
            ...linkBase,
            background: isActive ? "#f2f2f2" : "transparent",
          })}
        >
          Dashboard
        </NavLink>

        <NavLink
          to="/pessoas"
          style={({ isActive }) => ({
            ...linkBase,
            background: isActive ? "#f2f2f2" : "transparent",
          })}
        >
          Pessoas
        </NavLink>

        <NavLink
          to="/categorias"
          style={({ isActive }) => ({
            ...linkBase,
            background: isActive ? "#f2f2f2" : "transparent",
          })}
        >
          Categorias
        </NavLink>

        <NavLink
          to="/transacoes"
          style={({ isActive }) => ({
            ...linkBase,
            background: isActive ? "#f2f2f2" : "transparent",
          })}
        >
          Transações
        </NavLink>
      </nav>
    </aside>
  );
}
