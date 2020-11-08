#!/bin/bash

APP_SERVICES_ROOT_DIR="/var/www/EKTAIOFFICE/Services"
NGINX_ROOT_DIR="/etc/nginx";
NGINX_CONF_DIR="{{NGINX_CONF_DIR}}";

cp $NGINX_ROOT_DIR/includes/EKTAIOFFICE-communityserver-common.conf.template default-EKTAIOFFICE.conf;

sed 's/{{APP_NIGNX_KEEPLIVE}}/64/g' -i default-EKTAIOFFICE.conf;
sed '/mail\.default-api-scheme/s/\(value\s*=\s*\"\).*\"/\1http\"/' -i /var/www/EKTAIOFFICE/Services/MailAggregator/ASC.Mail.Aggregator.CollectionService.exe.config;

mv -f default-EKTAIOFFICE.conf ${NGINX_CONF_DIR}/EKTAIOFFICE.conf

sed '/certificate"/s!\(value\s*=\s*\"\).*\"!\1\"!' -i ${APP_SERVICES_ROOT_DIR}/TeamLabSvc/TeamLabSvc.exe.config
sed '/certificatePassword/s/\(value\s*=\s*\"\).*\"/\1\"/' -i ${APP_SERVICES_ROOT_DIR}/TeamLabSvc/TeamLabSvc.exe.config
sed '/startTls/s/\(value\s*=\s*\"\).*\"/\1none\"/' -i ${APP_SERVICES_ROOT_DIR}/TeamLabSvc/TeamLabSvc.exe.config;

systemctl restart EKTAIOFFICEMailAggregator
systemctl restart EKTAIOFFICEJabber
systemctl restart nginx