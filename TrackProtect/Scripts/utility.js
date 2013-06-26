var countryinfo = new Array(
    { country: 'Aaland Islands', languages: 'Swedish' },
    { country: 'Afghanistan', languages: 'Dari Persian, Pashto' },
    { country: 'Albania', languages: 'Albanian' },
    { country: 'Algeria', languages: 'Arabic' },
    { country: 'Andorra', languages: 'Catalan' },
    { country: 'American Samoa', languages: 'English' },
    { country: 'Angola', languages: 'Portuguese' },
    { country: 'Anguilla', languages: 'English' },
    { country: 'Antarctica', languages: 'English' },
    { country: 'Antigua and Barbuda', languages: 'English' },
    { country: 'Argentina', languages: 'Spanish' },
    { country: 'Armenia', languages: 'Armenian' },
    { country: 'Aruba', languages: 'Dutch, Papiamento' },
    { country: 'Australia', languages: 'English' },
    { country: 'Austria', languages: 'German' },
    { country: 'Azerbaijan', languages: 'Azerbaijani' },
    { country: 'Bahamas', languages: 'English' },
    { country: 'Bahrain', languages: 'Arabic' },
    { country: 'Bangladesh', languages: 'Bengali' },
    { country: 'Barbados', languages: 'English' },
    { country: 'Belarus', languages: 'Belarusian, Russian' },
    { country: 'Belgium', languages: 'Dutch, French, German' },
    { country: 'Belize', languages: 'English' },
    { country: 'Benin', languages: 'French' },
    { country: 'Bermuda', languages: 'English' },
    { country: 'Bhutan', languages: 'Dzongkha' },
    { country: 'Bolivia', languages: 'Spanish, Quechua, Aymara' },
    { country: 'Bosnia and Herzegowina', languages: 'Bosnian, Croatian and Serbian' },
    { country: 'Botswana', languages: 'English' },
    { country: 'Bouvet Island', languages: 'French' },
    { country: 'Brazil', languages: 'Portuguese' },
    { country: 'British Indian Ocean Territory', languages: 'English' },
    { country: 'Brunei Darussalam', languages: 'Malay' },
    { country: 'Bulgaria', languages: 'Bulgarian' },
    { country: 'Burkina Faso', languages: 'French' },
    { country: 'Burma', languages: 'Burmese' },
    { country: 'Burundi', languages: 'Kirundi, French' },
    { country: 'Cambodia', languages: 'Khmer' },
    { country: 'Cameroon', languages: 'English' },
    { country: 'Canada', languages: 'English' },
    { country: 'Cape Verde', languages: 'Portuguese' },
    { country: 'Cayman Islands', languages: 'English' },
    { country: 'Central African Republic', languages: 'Sango, French' },
    { country: 'Chad', languages: 'French, Arabic' },
    { country: 'Chile', languages: 'Spanish' },
    { country: 'China', languages: 'Chinese' },
    { country: 'Christmas Island', languages: 'English, Malay' },
    { country: 'Cocos (Keeling) Island', languages: 'English, Malay' },
    { country: 'Colombia', languages: 'Spanish' },
    { country: 'Comoros', languages: 'Comorian, Arabic, French' },
    { country: 'Congo, Democratic Republic of', languages: 'French' },
    { country: 'Congo, Republic of', languages: 'French' },
    { country: 'Cook Islands', languages: 'Maori, English' },
    { country: 'Costa Rica', languages: 'Spanish' },
    { country: 'Cote d\'Ivoire', languages: 'French' },
    { country: 'Croatia', languages: 'Croatian' },
    { country: 'Cuba', languages: 'Spanish' },
    { country: 'Cyprus', languages: 'Greek, Turkish' },
    { country: 'Czech Republic', languages: 'Czech' },
    { country: 'Denmark', languages: 'Danish' },
    { country: 'Djibouti', languages: 'Arabic, French' },
    { country: 'Dominica', languages: 'English' },
    { country: 'Dominican Republic', languages: 'Spanish' },
    { country: 'East Timor', languages: 'Portuguese' },
    { country: 'Ecuador', languages: 'Spanish' },
    { country: 'Egypt', languages: 'Arabic' },
    { country: 'El Salvador', languages: 'Spanish' },
    { country: 'Equatorial Guinea', languages: 'Spanish, French, Portuguese' },
    { country: 'Eritrea', languages: 'Tigrinya, Arabic and English' },
    { country: 'Estonia', languages: 'Estonian' },
    { country: 'Ethiopia', languages: 'Amharic' },
    { country: 'Falkland Islands (Malvinas)', languages: 'English' },
    { country: 'Faroe Islands', languages: 'Norwegian' },
    { country: 'Fiji', languages: 'English' },
    { country: 'Finland', languages: 'Finnish, Swedish' },
    { country: 'France', languages: 'French' },
    { country: 'French Guiana', languages: 'French' },
    { country: 'French Polynesia', languages: 'French' },
    { country: 'French Southern Territories', languages: 'French' },
    { country: 'Gabon', languages: 'French' },
    { country: 'Gambia', languages: 'English' },
    { country: 'Georgia', languages: 'Georgian' },
    { country: 'Germany', languages: 'German' },
    { country: 'Ghana', languages: 'English' },
    { country: 'Gibraltar', languages: 'English' },
    { country: 'Greece', languages: 'Greek' },
    { country: 'Greenland', languages: 'Greenlandic, Danish' },
    { country: 'Grenada', languages: 'English' },
    { country: 'Guadeloupe', languages: 'French' },
    { country: 'Guam', languages: 'Chamorro' },
    { country: 'Guatemala', languages: 'Spanish' },
    { country: 'Guinea Bissau', languages: 'Portuguese' },
    { country: 'Guinea', languages: 'French' },
    { country: 'Guyana', languages: 'English' },
    { country: 'Haiti', languages: 'French, Haitian Creole' },
    { country: 'Heard and McDonalds Islands', languages: 'English' },
    { country: 'Honduras', languages: 'Spanish' },
    { country: 'Hong Kong', languages: 'English, Manderin' },
    { country: 'Hungary', languages: 'Hungarian' },
    { country: 'Iceland', languages: 'Icelandic' },
    { country: 'India', languages: 'Hindi, English' },
    { country: 'Indonesia', languages: 'Indonesian' },
    { country: 'Iran', languages: 'Persian' },
    { country: 'Iraq', languages: 'Arabic, Kurdish, Aramaic' },
    { country: 'Ireland', languages: 'English' },
    { country: 'Israel', languages: 'Hebrew, Arabic' },
    { country: 'Italy', languages: 'Italian' },
    { country: 'Jamaica', languages: 'English' },
    { country: 'Japan', languages: 'Japanese' },
    { country: 'Jordan', languages: 'Arabic' },
    { country: 'Kazakhstan', languages: 'Kazakh, Russian' },
    { country: 'Kenya', languages: 'English' },
    { country: 'Kiribati', languages: 'Kiribati' },
    { country: 'Korea, Democratic People\'s Republic of', languages: 'Korean' },
    { country: 'Korea, Republic of', languages: 'Korean' },
    { country: 'Kuwait', languages: 'Arabic' },
    { country: 'Kyrgyzstan', languages: 'Kyrgyz, Russian' },
    { country: 'Laos', languages: 'Lao' },
    { country: 'Latvia', languages: 'Latvian' },
    { country: 'Lebanon', languages: 'Arabic' },
    { country: 'Lesotho', languages: 'English' },
    { country: 'Liberia', languages: 'English' },
    { country: 'Libyan Arab Jamahiriya', languages: 'Arabic' },
    { country: 'Liechtenstein', languages: 'German' },
    { country: 'Lithuania', languages: 'Lithuanian' },
    { country: 'Luxembourg', languages: 'French, German, Luxembourgish' },
    { country: 'Macao', languages: 'Portuguese' },
    { country: 'Macedonia', languages: 'Macedonian' },
    { country: 'Madagascar', languages: 'Malagasy, French, English' },
    { country: 'Malawi', languages: 'English, Chichewa' },
    { country: 'Malaysia', languages: 'Malay' },
    { country: 'Maldives', languages: 'Dhivehi' },
    { country: 'Mali', languages: 'French' },
    { country: 'Malta', languages: 'English, Maltese' },
    { country: 'Marshall Islands', languages: 'English, Marshallese' },
    { country: 'Martinique', languages: 'French, Antillean Creole' },
    { country: 'Mauritania', languages: 'Arabic' },
    { country: 'Mauritius', languages: 'English' },
    { country: 'Mayotte', languages: 'French, Swahili, Malagasy' },
    { country: 'Mexico', languages: 'Spanish' },
    { country: 'Micronesia, Federated States of', languages: 'Chamorro, Palauan' },
    { country: 'Moldova, Republic of', languages: 'Moldovan' },
    { country: 'Monaco', languages: 'French' },
    { country: 'Mongolia', languages: 'Mongolian' },
    { country: 'Montserrat', languages: 'English' },
    { country: 'Morocco', languages: 'Arabic' },
    { country: 'Mozambique', languages: 'Portuguese' },
    { country: 'Myanmar', languages: 'Burmese, English' },
    { country: 'Namibia', languages: 'English' },
    { country: 'Nauru', languages: 'Nauruan' },
    { country: 'Nepal', languages: 'Nepali' },
    { country: 'Netherlands', languages: 'Dutch' },
    { country: 'Netherlands Antilles', languages: 'Dutch, English, Papiamento' },
    { country: 'New Caledonia', languages: 'French' },
    { country: 'New zealand', languages: 'English' },
    { country: 'Nicaragua', languages: 'Spanish' },
    { country: 'Niger', languages: 'French' },
    { country: 'Nigeria', languages: 'English' },
    { country: 'Niue', languages: 'Niuean' },
    { country: 'Norfolk Island', languages: 'English' },
    { country: 'Northern Mariana Islands', languages: 'English, Chamorro, Carolinian' },
    { country: 'Norway', languages: 'Norwegian' },
    { country: 'Oman', languages: 'Arabic' },
    { country: 'Pakistan', languages: 'Urdu' },
    { country: 'Palau', languages: 'Palauan, English, Japanese' },
    { country: 'Palestinian Territory, Occupied', languages: 'English' },
    { country: 'Panama', languages: 'Spanish' },
    { country: 'Papua New Guinea', languages: 'English, Tok Pisin, Hiri Motu' },
    { country: 'Paraguay', languages: 'Spanish, Guaraní' },
    { country: 'Peru', languages: 'Spanish' },
    { country: 'Philippines', languages: 'Filipino, English' },
    { country: 'Pitcairn', languages: 'English, Pitkern' },
    { country: 'Poland', languages: 'Polish' },
    { country: 'Portugal', languages: 'Portuguese' },
    { country: 'Puerto Rico', languages: 'Spanish, English, French' },
    { country: 'Qatar', languages: 'Arabic' },
    { country: 'Reunion', languages: 'French' },
    { country: 'Romania', languages: 'Romanian' },
    { country: 'Russian Federation', languages: 'Russian' },
    { country: 'Rwanda', languages: 'Kinyarwanda, French, English' },
    { country: 'Saint Helena', languages: 'English' },
    { country: 'Saint Kitts and Nevis', languages: 'English' },
    { country: 'Saint Lucia', languages: 'English, French Creole' },
    { country: 'Saint Pierre and Miquelon', languages: 'French' },
    { country: 'Saint Vincent and the Grenadines', languages: 'English' },
    { country: 'Samoa', languages: 'Samoan, English' },
    { country: 'San Marino', languages: 'Italian' },
    { country: 'Sao Principe and Tome', languages: 'Portuguese' },
    { country: 'Saudi Arabia', languages: 'Arabic' },
    { country: 'Senegal', languages: 'French' },
    { country: 'Serbia and Montenegro', languages: 'Serbian' },
    { country: 'Seychelles', languages: 'English, French, Seychellois Creole' },
    { country: 'Sierra Leone', languages: 'English' },
    { country: 'Singapore', languages: 'English, Mandarin' },
    { country: 'Slovakia', languages: 'Slovak' },
    { country: 'Slovenia', languages: 'Slovene' },
    { country: 'Solomon Islands', languages: 'English' },
    { country: 'Somali', languages: 'Somali' },
    { country: 'South Africa', languages: 'Afrikaans, English, Southern Ndebele, Northern Sotho, Sotho, Swati, Tsonga, Tswana, Venda, Xhosa, Zulu' },
    { country: 'South Georgia and the South Sandwich Islands', languages: 'English' },
    { country: 'Spain', languages: 'Spanish' },
    { country: 'Sri lanka', languages: 'Sinhala, Tamil' },
    { country: 'Sudan', languages: 'Arabic' },
    { country: 'Suriname', languages: 'Dutch' },
    { country: 'Svalbard and Jan Mayen islands', languages: 'Norwegian' },
    { country: 'Swaziland', languages: 'English, Swati' },
    { country: 'Sweden', languages: 'Swedish' },
    { country: 'Switzerland', languages: 'German, French, Italian, Romansh' },
    { country: 'Syrian Arab Republic', languages: 'Arabic' },
    { country: 'Taiwan', languages: 'Chinese' },
    { country: 'Tajikistan', languages: 'Tajik, Russian' },
    { country: 'Tanzania, United Republic of', languages: 'Swahili' },
    { country: 'Thailand', languages: 'Thai' },
    { country: 'Timor-Leste', languages: 'Portuguese, English, Tetum' },
    { country: 'Togo', languages: 'French' },
    { country: 'Tokelau', languages: 'Tokelauan' },
    { country: 'Tonga', languages: 'Tongan' },
    { country: 'Trinidad and Tobago', languages: 'English' },
    { country: 'Tunisia', languages: 'Arabic' },
    { country: 'Turkey', languages: 'Turkish' },
    { country: 'Turkmenistan', languages: 'Turkmen' },
    { country: 'Turks and Caicos Islands', languages: 'English' },
    { country: 'Tuvalu', languages: 'Tuvalu' },
    { country: 'Uganda', languages: 'English, Swahili' },
    { country: 'Ukraine', languages: 'Ukrainian' },
    { country: 'United Arab Emirates', languages: 'Arabic' },
    { country: 'United kingdom', languages: 'English' },
    { country: 'United States', languages: 'English' },
    { country: 'United States Minor Outlying Islands', languages: 'English' },
    { country: 'Uruguay', languages: 'Spanish' },
    { country: 'Uzbekistan', languages: 'Uzbek' },
    { country: 'Vanuatu', languages: 'English' },
    { country: 'Vatican City State (Holy See)', languages: 'Italian' },
    { country: 'Venezuela', languages: 'Spanish' },
    { country: 'Vietnam', languages: 'Vietnamese' },
    { country: 'Virgin Islands (British)', languages: 'English' },
    { country: 'Virgin Islands (U.S.)', languages: 'English' },
    { country: 'Wallis and Futuna Islands', languages: 'French' },
    { country: 'Western Sahara', languages: 'Arabic, Berber, Spanish, French' },
    { country: 'Yemen', languages: 'Arabic' },
    { country: 'Zambia', languages: 'English' },
    { country: 'Zimbabwe', languages: 'English' }
);

function fillCountryList(listname) {
    var con = document.getElementById(listname);
    con.innerHTML = "";
    var theOption = new Option;
    theOption.text = "Select Country";
    theOption.value = -1;
    con.add(theOption, null);
    for (var i = 0; i < countryinfo.length; i++) {
        theOption = new Option;
        theOption.text = countryinfo[i].country;
        theOption.value = i;
        con.add(theOption, null);
    }
}

function fillLanguageList(list, hiddenfield, index) {
    var con = list;
    con.innerHTML = "";
    if (index == -1) {
        con.value = "";
        return;
    }
    var lan = countryinfo[index].languages;
    var languages = lan.split(", ");
    for (i = 0; i < languages.length; i++) {
        var theOption = new Option;
        theOption.text = languages[i];
        theOption.value = languages[i];
        con.add(theOption, null);
    }
    con.selectedindex = 0;
    hiddenfield.value = con.options[con.selectedindex].text;
}

function fillLanguageListEdit(list, hiddenfield, index) {
    var con = list;
    con.innerHTML = "";
    if (index == -1) {
        con.value = "";
        return;
    }
    var lan = countryinfo[index].languages;
    var languages = lan.split(", ");
    for (i = 0; i < languages.length; i++) {
        var theOption = new Option;
        theOption.text = languages[i];
        theOption.value = languages[i];
        con.add(theOption, null);
    }
    con.selectedindex = 0;
    try {
        if (hiddenfield.value != "") {
            for (var i = 0; i < con.options.length; i++) {
                if (con.options[i].text == hiddenfield.value) {
                    con.selectedindex = i;
                    break;
                }
            }
        }
        hiddenfield.value = con.options[con.selectedindex].text;
        storeSelection(con, hiddenfield);
    } catch (err) {

    }
}

function storeSelection(listelem, hiddenfield) {

    if (!listelem || !hiddenfield) {
        return;
    }

    hiddenfield.value = listelem.options[listelem.selectedindex].text;
}

function moveToList(dropDownList, listBox) {
    var index = dropDownList.selectedIndex;
    if (index > -1) {
        var text = dropDownList.options[index].text;
        var value = dropDownList.options[index].value;
        var opt = document.createElement("option");
        opt.text = text;
        opt.value = value;
        listBox.options.add(opt);
    }
}
