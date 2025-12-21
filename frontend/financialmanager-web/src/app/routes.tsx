import { Routes, Route } from "react-router-dom";
import AppLayout from "../components/Layout/AppLayout";
import DashboardPage from "../pages/Dashboard/DashboardPage";

function Placeholder({ title }: { title: string }) {
  return <h2 style={{ margin: 0 }}>{title} (em breve)</h2>;
}

export default function AppRoutes() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route path="/" element={<DashboardPage />} />
        <Route path="/pessoas" element={<Placeholder title="Pessoas" />} />
        <Route path="/categorias" element={<Placeholder title="Categorias" />} />
        <Route path="/transacoes" element={<Placeholder title="Transações" />} />
      </Route>
    </Routes>
  );
}
