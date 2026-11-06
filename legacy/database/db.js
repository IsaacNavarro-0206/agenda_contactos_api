import Sequelize from 'sequelize';
import Dotenv from 'dotenv';
Dotenv.config({ path: 'variables.env' });

const db = new Sequelize(process.env.DB_NOMBRE, process.env.DB_USER, process.env.DB_PASSWORD, {
    host: process.env.DB_HOST,
    port: process.env.DB_PORT,
    dialect: 'mysql',
    define: {
        timestamps: false
    }
});

export default db;