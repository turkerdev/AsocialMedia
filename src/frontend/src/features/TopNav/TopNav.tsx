import { Link } from "@remix-run/react";

export function TopNav() {
  return (
    <div className="border-b px-4">
      <Link to="/resource">Resource</Link>
    </div>
  );
}
